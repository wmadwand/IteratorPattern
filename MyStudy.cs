using System;
using System.Collections;

namespace IteratorMS01
{
    // element of the collection
    public class Book
    {
        public string Name;

        public Book(string name)
        {
            Name = name;
        }
    }

    // iterator
    public interface IBookIterator
    {
        Book First();
        Book Next();
        bool IsDone { get; }
        Book CurrentItem { get; }
    }

    // aggregate
    public interface IBookCollection
    {
        IBookIterator CreateIterator(IIteratorFactory factory, BookIteratorType type);
        int Count { get; }
        Book this[int index] { get; set; }
    }

    // concrete iterator
    public class LibraryIterator : IBookIterator
    {
        private IBookCollection _collection;
        private int _current;

        public LibraryIterator(IBookCollection collection)
        {
            _collection = collection;
        }

        public Book First()
        {
            _current = 0;
            return _collection[_current];
        }

        public Book Next()
        {
            _current++;
            return !IsDone ? _collection[_current] : null;
        }

        public bool IsDone { get { return _current >= _collection.Count; } }

        public Book CurrentItem { get { return _collection[_current]; } }
    }

    public class LibraryReverseIterator : IBookIterator
    {
        private IBookCollection _collection;
        private int _current;

        public LibraryReverseIterator(IBookCollection collection)
        {
            _collection = collection;
        }

        public bool IsDone { get { return _current < 0; } }

        public Book CurrentItem { get { return _collection[_current]; } }

        public Book First()
        {
            _current = _collection.Count - 1;
            return _collection[_current];
        }

        public Book Next()
        {
            _current--;
            return !IsDone ? _collection[_current] : null;
        }
    }

    public enum BookIteratorType
    {
        normal,
        reverse
    }

    public interface IIteratorFactory
    {
        IBookIterator Create(IBookCollection collection, BookIteratorType type);
    }

    public class IteratorFactory : IIteratorFactory
    {
        public virtual IBookIterator Create(IBookCollection collection, BookIteratorType type)
        {
            IBookIterator iterator = null;

            switch (type)
            {
                case BookIteratorType.normal:
                    iterator = new LibraryIterator(collection);
                    break;
                case BookIteratorType.reverse:
                    iterator = new LibraryReverseIterator(collection);
                    break;
                default:
                    iterator = new LibraryIterator(collection);
                    break;
            }

            return iterator;
        }
    }

    // concrete aggregate
    public class Library : IBookCollection
    {
        private ArrayList _items = new ArrayList();

        public IBookIterator CreateIterator(IIteratorFactory factory, BookIteratorType type)
        {
            return factory.Create(this, type);
        }

        public int Count { get { return _items.Count; } }

        public Book this[int index]
        {
            get { return _items[index] as Book; }
            set { _items.Add(value); }
        }
    }

    public class Reader
    {
        public void SeeBooks(IBookCollection library, IBookIterator iterator)
        {
            for (Book item = iterator.First(); !iterator.IsDone; item = iterator.Next())
            {
                Console.WriteLine(iterator.CurrentItem.Name);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IBookCollection library = new Library();
            IIteratorFactory iteratorFactory = new IteratorFactory();

            IBookIterator iteratorNormal = library.CreateIterator(iteratorFactory, BookIteratorType.normal);
            IBookIterator iteratorReverse = library.CreateIterator(iteratorFactory, BookIteratorType.reverse);

            library[0] = new Book("book01");
            library[1] = new Book("book02");

            Reader reader = new Reader();
            reader.SeeBooks(library, iteratorNormal);
            reader.SeeBooks(library, iteratorReverse);

            Console.ReadKey();
        }
    }
}
