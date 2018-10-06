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
        IBookIterator CreateIterator();
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

    // concrete aggregate
    public class Library : IBookCollection
    {
        private ArrayList _items = new ArrayList();

        public IBookIterator CreateIterator()
        {
            return new LibraryIterator(this);
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
        public void SeeBooks(Library library)
        {
            IBookIterator iterator = library.CreateIterator();

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
            Library library = new Library();

            library[0] = new Book("book01");
            library[1] = new Book("book02");

            Reader reader = new Reader();
            reader.SeeBooks(library);

            Console.ReadKey();
        }
    }
}
