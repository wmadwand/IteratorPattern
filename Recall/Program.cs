using System;
using System.Collections;

namespace Recall
{
    public class Book
    {
        private string _name;

        public Book(string name)
        {
            _name = name;
        }

        public void Print()
        {
            Console.WriteLine(_name);
        }
    }

    public interface IBookIterator
    {
        Book First();
        Book Next();
        bool IsDone { get; }
        Book CurrentItem { get; }
    }

    public class LibraryIterator : IBookIterator
    {
        private IBookCollection _collection;
        private int _current;

        public LibraryIterator(IBookCollection collection)
        {
            _collection = collection;
        }

        Book IBookIterator.First()
        {
            _current = 0;
            return _collection[_current];
        }

        Book IBookIterator.Next()
        {
            _current++;
            return !IsDone ? _collection[_current] : null;
        }

        public bool IsDone => _current >= _collection.Count;

        Book IBookIterator.CurrentItem => _collection[_current];
    }

    public class LibraryReverseIterator : IBookIterator
    {
        private IBookCollection _collection;
        private int _current;

        public LibraryReverseIterator(IBookCollection collection)
        {
            _collection = collection;
        }

        Book IBookIterator.First()
        {
            _current = _collection.Count - 1;
            return _collection[_current];
        }

        Book IBookIterator.Next()
        {
            _current--;
            return !IsDone ? _collection[_current] : null;
        }

        public bool IsDone => _current < 0;

        Book IBookIterator.CurrentItem => _collection[_current];
    }

    public interface IBookCollection
    {
        IBookIterator CreateIterator(IIteratorFactory factory, IteratorType iteratorType);
        int Count { get; }
        Book this[int index] { get; set; }
    }

    public class Library : IBookCollection
    {
        private ArrayList _items = new ArrayList();

        Book IBookCollection.this[int index] { get => _items[index] as Book; set => _items.Add(value); }

        int IBookCollection.Count => _items.Count;

        IBookIterator IBookCollection.CreateIterator(IIteratorFactory factory, IteratorType iteratorType)
        {
            return factory.Create(this, iteratorType);
        }
    }

    public enum IteratorType { Normal, Reverse }

    public interface IIteratorFactory
    {
        IBookIterator Create(IBookCollection col, IteratorType iteratorType);
    }

    public class IteratorFactory : IIteratorFactory
    {
        IBookIterator IIteratorFactory.Create(IBookCollection col, IteratorType iteratorType)
        {
            IBookIterator iterator = null;

            switch (iteratorType)
            {
                case IteratorType.Normal: iterator = new LibraryIterator(col); break;

                case IteratorType.Reverse: iterator = new LibraryReverseIterator(col); break;
            }

            return iterator;
        }
    }

    public class Reader
    {
        public void SeeBooks(IBookIterator iterator)
        {
            for (Book book = iterator.First(); !iterator.IsDone; book = iterator.Next())
            {
                book.Print();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IBookCollection library = new Library();

            library[0] = new Book("Book01");
            library[1] = new Book("Book02");

            IIteratorFactory factory = new IteratorFactory();

            IBookIterator itNormal = library.CreateIterator(factory, IteratorType.Normal);
            IBookIterator itReverse = library.CreateIterator(factory, IteratorType.Reverse);


            Reader reader = new Reader();

            reader.SeeBooks(itNormal);
            reader.SeeBooks(itReverse);
        }
    }
}
